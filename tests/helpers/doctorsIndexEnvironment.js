const fs = require('fs');
const path = require('path');
const { JSDOM, VirtualConsole } = require('jsdom');

const VIEW_PATH = path.resolve(__dirname, '../../Views/Doctors/Index.cshtml');

function readViewSource() {
  return fs.readFileSync(VIEW_PATH, 'utf8');
}

function extractBodyMarkup(rawHtml) {
  // Strip the leading Razor code block: @{ ... }
  return rawHtml.replace(/^@\{[\s\S]*?\}\s*/, '');
}

function flushPromises() {
  return new Promise((resolve) => setImmediate(resolve));
}

/**
 * Boots a jsdom environment using the real markup + inline script extracted
 * from Views/Doctors/Index.cshtml (scripts execute for real via
 * `runScripts: "dangerously"`), with fetch/bootstrap/alert mocked and the
 * jwt token seeded into localStorage before the script runs.
 *
 * @param {object} [options]
 * @param {string|null} [options.token] - value returned by localStorage.getItem('jwt'). Pass null to simulate a logged-out user.
 * @param {object} [options.initialFetchResponse] - the response the automatic bottom-of-script `loadDoctors()` call should resolve with.
 */
async function createDoctorsIndexEnvironment(options = {}) {
  const { token = 'test-token', initialFetchResponse = { success: true, data: [] } } = options;

  const rawHtml = readViewSource();
  const bodyMarkup = extractBodyMarkup(rawHtml);

  const jsdomErrors = [];
  const virtualConsole = new VirtualConsole();
  virtualConsole.on('jsdomError', (err) => jsdomErrors.push(err));

  const modalInstance = { show: jest.fn(), hide: jest.fn() };
  const ModalCtor = jest.fn().mockImplementation(() => modalInstance);
  ModalCtor.getInstance = jest.fn(() => modalInstance);

  const fetchMock = jest.fn().mockResolvedValue({
    ok: true,
    json: async () => initialFetchResponse,
  });
  const alertMock = jest.fn();

  const dom = new JSDOM(`<!DOCTYPE html><html><body>${bodyMarkup}</body></html>`, {
    url: 'http://localhost/Doctors',
    virtualConsole,
    runScripts: 'dangerously',
    beforeParse(window) {
      if (token !== null && token !== undefined) {
        window.localStorage.setItem('jwt', token);
      }
      window.fetch = fetchMock;
      window.alert = alertMock;
      window.bootstrap = { Modal: ModalCtor };
    },
  });

  const { window } = dom;

  // The script calls `loadDoctors()` unconditionally as its final statement,
  // which runs synchronously as the <script> tag is parsed. Let that initial
  // call settle, then reset mock call history so tests start from a clean slate.
  await flushPromises();
  fetchMock.mockClear();
  alertMock.mockClear();
  ModalCtor.mockClear();
  ModalCtor.getInstance.mockClear();
  modalInstance.show.mockClear();
  modalInstance.hide.mockClear();

  return {
    window,
    document: window.document,
    jsdomErrors,
    modalInstance,
    ModalCtor,
    fetchMock,
    alertMock,
  };
}

module.exports = {
  createDoctorsIndexEnvironment,
  extractBodyMarkup,
  readViewSource,
  flushPromises,
  VIEW_PATH,
};