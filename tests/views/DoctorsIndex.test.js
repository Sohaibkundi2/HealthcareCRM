const { createDoctorsIndexEnvironment, flushPromises } = require('../helpers/doctorsIndexEnvironment');

/**
 * These tests exercise the inline <script> from Views/Doctors/Index.cshtml
 * inside a real jsdom document built from the actual markup in that file,
 * with fetch/bootstrap/alert mocked. This keeps the tests tied to the real
 * source so that regressions in the view are caught.
 */
describe('Views/Doctors/Index.cshtml inline script', () => {
  describe('authentication redirect', () => {
    it('redirects to /Account/Login when no jwt token is present', async () => {
      const { jsdomErrors } = await createDoctorsIndexEnvironment({ token: null });

      expect(
        jsdomErrors.some((e) => /Not implemented: navigation/.test(e.message))
      ).toBe(true);
    });

    it('does not attempt to navigate away when a jwt token is present', async () => {
      const { jsdomErrors } = await createDoctorsIndexEnvironment({ token: 'valid-token' });

      expect(
        jsdomErrors.some((e) => /Not implemented: navigation/.test(e.message))
      ).toBe(false);
    });
  });

  describe('escapeHtml', () => {
    it('escapes HTML special characters', async () => {
      const { window } = await createDoctorsIndexEnvironment();

      expect(window.escapeHtml('<script>alert(1)</script>')).toBe(
        '&lt;script&gt;alert(1)&lt;/script&gt;'
      );
    });

    it('returns an empty string for null, undefined or empty input', async () => {
      const { window } = await createDoctorsIndexEnvironment();

      expect(window.escapeHtml(null)).toBe('');
      expect(window.escapeHtml(undefined)).toBe('');
      expect(window.escapeHtml('')).toBe('');
    });

    it('leaves plain text untouched', async () => {
      const { window } = await createDoctorsIndexEnvironment();

      expect(window.escapeHtml('Dr. Jane Roe')).toBe('Dr. Jane Roe');
    });
  });

  describe('loadDoctors', () => {
    it('requests active-only doctors by default with the auth header', async () => {
      const { window, fetchMock } = await createDoctorsIndexEnvironment({ token: 'abc123' });
      fetchMock.mockResolvedValueOnce({ ok: true, json: async () => ({ success: true, data: [] }) });

      await window.loadDoctors();

      expect(fetchMock).toHaveBeenCalledWith('/api/doctors', {
        headers: { Authorization: 'Bearer abc123' },
      });
    });

    it('requests all doctors (including inactive) after toggleInactive(true)', async () => {
      const { window, fetchMock } = await createDoctorsIndexEnvironment({ token: 'abc123' });
      fetchMock.mockResolvedValue({ ok: true, json: async () => ({ success: true, data: [] }) });

      window.toggleInactive(true);
      await flushPromises();

      expect(fetchMock).toHaveBeenLastCalledWith('/api/doctors?includeInactive=true', {
        headers: { Authorization: 'Bearer abc123' },
      });
    });

    it('renders a table row per doctor with escaped content and correct status badge', async () => {
      const { window, document, fetchMock } = await createDoctorsIndexEnvironment();
      fetchMock.mockResolvedValueOnce({
        ok: true,
        json: async () => ({
          success: true,
          data: [
            { id: 1, name: 'Dr. <b>Active</b>', specialization: 'Cardiology', phone: '111', isActive: true },
            { id: 2, name: 'Dr. Inactive', specialization: 'Dermatology', phone: '222', isActive: false },
          ],
        }),
      });

      await window.loadDoctors();

      const rows = document.querySelectorAll('#doctors-table tr');
      expect(rows.length).toBe(2);
      expect(rows[0].innerHTML).toContain('&lt;b&gt;Active&lt;/b&gt;');
      expect(rows[0].textContent).toContain('Active');
      expect(rows[0].innerHTML).toContain('Deactivate');
      expect(rows[1].textContent).toContain('Inactive');
      expect(rows[1].innerHTML).toContain('Reactivate');

      expect(document.getElementById('loading').classList.contains('d-none')).toBe(true);
      expect(document.getElementById('doctors-card').classList.contains('d-none')).toBe(false);
      expect(document.getElementById('empty-state').classList.contains('d-none')).toBe(true);
    });

    it('shows the empty state and clears the table when there are no doctors', async () => {
      const { window, document, fetchMock } = await createDoctorsIndexEnvironment();
      document.getElementById('doctors-table').innerHTML = '<tr><td>stale row</td></tr>';
      fetchMock.mockResolvedValueOnce({ ok: true, json: async () => ({ success: true, data: [] }) });

      await window.loadDoctors();

      expect(document.getElementById('empty-state').classList.contains('d-none')).toBe(false);
      expect(document.getElementById('doctors-table').innerHTML).toBe('');
    });

    it('shows an error message in the loading area when the request fails', async () => {
      const { window, document, fetchMock } = await createDoctorsIndexEnvironment();
      fetchMock.mockResolvedValueOnce({ ok: false, json: async () => ({ success: false }) });

      await window.loadDoctors();

      expect(document.getElementById('loading').innerHTML).toContain('Failed to load doctors');
    });

    it('shows an error message in the loading area when fetch rejects', async () => {
      const { window, document, fetchMock } = await createDoctorsIndexEnvironment();
      fetchMock.mockRejectedValueOnce(new Error('network down'));

      await window.loadDoctors();

      expect(document.getElementById('loading').innerHTML).toContain('Failed to load doctors');
    });
  });

  describe('showAddForm / showEditForm', () => {
    it('resets the modal fields and title for adding a new doctor', async () => {
      const { window, document, modalInstance, ModalCtor } = await createDoctorsIndexEnvironment();
      document.getElementById('name').value = 'leftover';

      window.showAddForm();

      expect(document.getElementById('modal-title').textContent).toBe('Add Doctor');
      expect(document.getElementById('doctor-id').value).toBe('');
      expect(document.getElementById('name').value).toBe('');
      expect(document.getElementById('specialization').value).toBe('');
      expect(document.getElementById('phone').value).toBe('');
      expect(document.getElementById('form-error').classList.contains('d-none')).toBe(true);
      expect(ModalCtor).toHaveBeenCalledWith(document.getElementById('doctorModal'));
      expect(modalInstance.show).toHaveBeenCalled();
    });

    it('populates the modal fields with the given doctor for editing', async () => {
      const { window, document } = await createDoctorsIndexEnvironment();

      window.showEditForm({ id: 7, name: 'Dr. Strange', specialization: 'Neurology', phone: '555' });

      expect(document.getElementById('modal-title').textContent).toBe('Edit Doctor');
      expect(document.getElementById('doctor-id').value).toBe('7');
      expect(document.getElementById('name').value).toBe('Dr. Strange');
      expect(document.getElementById('specialization').value).toBe('Neurology');
      expect(document.getElementById('phone').value).toBe('555');
    });
  });

  describe('saveDoctor', () => {
    async function fillForm(document, { id = '', name = '', specialization = '', phone = '' } = {}) {
      document.getElementById('doctor-id').value = id;
      document.getElementById('name').value = name;
      document.getElementById('specialization').value = specialization;
      document.getElementById('phone').value = phone;
    }

    it('shows a validation error and does not call fetch when name is missing', async () => {
      const { window, document, fetchMock } = await createDoctorsIndexEnvironment();
      await fillForm(document, { specialization: 'Cardiology', phone: '111' });

      await window.saveDoctor();

      expect(document.getElementById('form-error').textContent).toBe('Name is required');
      expect(document.getElementById('form-error').classList.contains('d-none')).toBe(false);
      expect(fetchMock).not.toHaveBeenCalled();
    });

    it('shows a validation error and does not call fetch when specialization is missing', async () => {
      const { window, document, fetchMock } = await createDoctorsIndexEnvironment();
      await fillForm(document, { name: 'Dr. Roe', phone: '111' });

      await window.saveDoctor();

      expect(document.getElementById('form-error').textContent).toBe('Specialization is required');
      expect(fetchMock).not.toHaveBeenCalled();
    });

    it('shows a validation error and does not call fetch when phone is missing', async () => {
      const { window, document, fetchMock } = await createDoctorsIndexEnvironment();
      await fillForm(document, { name: 'Dr. Roe', specialization: 'Cardiology' });

      await window.saveDoctor();

      expect(document.getElementById('form-error').textContent).toBe('Phone is required');
      expect(fetchMock).not.toHaveBeenCalled();
    });

    it('POSTs to /api/doctors when there is no doctor id (create)', async () => {
      const { window, document, fetchMock, modalInstance } = await createDoctorsIndexEnvironment({ token: 'abc123' });
      await fillForm(document, { name: 'Dr. Roe', specialization: 'Cardiology', phone: '111' });
      fetchMock.mockResolvedValueOnce({ ok: true, json: async () => ({ success: true }) });
      fetchMock.mockResolvedValueOnce({ ok: true, json: async () => ({ success: true, data: [] }) }); // reload

      await window.saveDoctor();

      expect(fetchMock).toHaveBeenNthCalledWith(1, '/api/doctors', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', Authorization: 'Bearer abc123' },
        body: JSON.stringify({ name: 'Dr. Roe', specialization: 'Cardiology', phone: '111' }),
      });
      expect(modalInstance.hide).toHaveBeenCalled();
    });

    it('PUTs to /api/doctors/{id} when a doctor id is present (update)', async () => {
      const { window, document, fetchMock } = await createDoctorsIndexEnvironment({ token: 'abc123' });
      await fillForm(document, { id: '42', name: 'Dr. Roe', specialization: 'Cardiology', phone: '111' });
      fetchMock.mockResolvedValueOnce({ ok: true, json: async () => ({ success: true }) });
      fetchMock.mockResolvedValueOnce({ ok: true, json: async () => ({ success: true, data: [] }) }); // reload

      await window.saveDoctor();

      expect(fetchMock).toHaveBeenNthCalledWith(1, '/api/doctors/42', {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json', Authorization: 'Bearer abc123' },
        body: JSON.stringify({ name: 'Dr. Roe', specialization: 'Cardiology', phone: '111' }),
      });
    });

    it('trims whitespace from the submitted fields', async () => {
      const { window, document, fetchMock } = await createDoctorsIndexEnvironment();
      await fillForm(document, { name: '  Dr. Roe  ', specialization: ' Cardiology ', phone: ' 111 ' });
      fetchMock.mockResolvedValueOnce({ ok: true, json: async () => ({ success: true }) });
      fetchMock.mockResolvedValueOnce({ ok: true, json: async () => ({ success: true, data: [] }) });

      await window.saveDoctor();

      const [, requestInit] = fetchMock.mock.calls[0];
      expect(JSON.parse(requestInit.body)).toEqual({
        name: 'Dr. Roe',
        specialization: 'Cardiology',
        phone: '111',
      });
    });

    it('shows the server error message and keeps the modal open when the request fails', async () => {
      const { window, document, fetchMock, modalInstance } = await createDoctorsIndexEnvironment();
      await fillForm(document, { name: 'Dr. Roe', specialization: 'Cardiology', phone: '111' });
      fetchMock.mockResolvedValueOnce({ ok: false, json: async () => ({ success: false, message: 'Phone already exists' }) });

      await window.saveDoctor();

      expect(document.getElementById('form-error').textContent).toBe('Phone already exists');
      expect(document.getElementById('form-error').classList.contains('d-none')).toBe(false);
      expect(modalInstance.hide).not.toHaveBeenCalled();
    });

    it('shows a generic error message when fetch throws', async () => {
      const { window, document, fetchMock } = await createDoctorsIndexEnvironment();
      await fillForm(document, { name: 'Dr. Roe', specialization: 'Cardiology', phone: '111' });
      fetchMock.mockRejectedValueOnce(new Error('network down'));

      await window.saveDoctor();

      expect(document.getElementById('form-error').textContent).toBe('Something went wrong. Please try again.');
    });

    it('re-enables the save button with its original label after saving', async () => {
      const { window, document, fetchMock } = await createDoctorsIndexEnvironment();
      await fillForm(document, { name: 'Dr. Roe', specialization: 'Cardiology', phone: '111' });
      fetchMock.mockResolvedValueOnce({ ok: true, json: async () => ({ success: true }) });
      fetchMock.mockResolvedValueOnce({ ok: true, json: async () => ({ success: true, data: [] }) });

      await window.saveDoctor();

      const saveBtn = document.getElementById('save-btn');
      expect(saveBtn.disabled).toBe(false);
      expect(saveBtn.textContent).toBe('Save');
    });
  });

  describe('showToggleConfirm / confirmToggle', () => {
    it('populates the confirmation modal for deactivating an active doctor', async () => {
      const { window, document, modalInstance } = await createDoctorsIndexEnvironment();

      window.showToggleConfirm(5, true, 'Dr. Roe');

      expect(document.getElementById('deactivate-title').textContent).toBe('Deactivate Doctor');
      expect(document.getElementById('deactivate-body').textContent).toContain('deactivate Dr. Roe');
      expect(document.getElementById('deactivate-btn').textContent).toBe('Deactivate');
      expect(document.getElementById('deactivate-btn').className).toContain('btn-danger');
      expect(modalInstance.show).toHaveBeenCalled();
    });

    it('populates the confirmation modal for reactivating an inactive doctor', async () => {
      const { window, document } = await createDoctorsIndexEnvironment();

      window.showToggleConfirm(5, false, 'Dr. Roe');

      expect(document.getElementById('deactivate-title').textContent).toBe('Reactivate Doctor');
      expect(document.getElementById('deactivate-body').textContent).toContain('reactivate Dr. Roe');
      expect(document.getElementById('deactivate-btn').textContent).toBe('Reactivate');
      expect(document.getElementById('deactivate-btn').className).toContain('btn-success');
    });

    it('does nothing when confirmToggle is called without a prior target', async () => {
      const { window, fetchMock } = await createDoctorsIndexEnvironment();

      await window.confirmToggle();

      expect(fetchMock).not.toHaveBeenCalled();
    });

    it('PUTs to the deactivate endpoint for an active doctor and reloads the list', async () => {
      const { window, fetchMock, modalInstance } = await createDoctorsIndexEnvironment({ token: 'abc123' });
      window.showToggleConfirm(5, true, 'Dr. Roe');
      fetchMock.mockResolvedValueOnce({ ok: true, json: async () => ({ success: true }) });
      fetchMock.mockResolvedValueOnce({ ok: true, json: async () => ({ success: true, data: [] }) }); // reload

      await window.confirmToggle();

      expect(fetchMock).toHaveBeenNthCalledWith(1, '/api/doctors/5/deactivate', {
        method: 'PUT',
        headers: { Authorization: 'Bearer abc123' },
      });
      expect(modalInstance.hide).toHaveBeenCalled();
    });

    it('PUTs to the reactivate endpoint for an inactive doctor', async () => {
      const { window, fetchMock } = await createDoctorsIndexEnvironment({ token: 'abc123' });
      window.showToggleConfirm(9, false, 'Dr. Roe');
      fetchMock.mockResolvedValueOnce({ ok: true, json: async () => ({ success: true }) });
      fetchMock.mockResolvedValueOnce({ ok: true, json: async () => ({ success: true, data: [] }) });

      await window.confirmToggle();

      expect(fetchMock).toHaveBeenNthCalledWith(1, '/api/doctors/9/reactivate', {
        method: 'PUT',
        headers: { Authorization: 'Bearer abc123' },
      });
    });

    it('clears the pending target after a successful toggle so it cannot run twice', async () => {
      const { window, fetchMock } = await createDoctorsIndexEnvironment();
      window.showToggleConfirm(5, true, 'Dr. Roe');
      fetchMock.mockResolvedValueOnce({ ok: true, json: async () => ({ success: true }) });
      fetchMock.mockResolvedValueOnce({ ok: true, json: async () => ({ success: true, data: [] }) });

      await window.confirmToggle();
      fetchMock.mockClear();
      await window.confirmToggle();

      expect(fetchMock).not.toHaveBeenCalled();
    });

    it('alerts the user when the toggle request fails', async () => {
      const { window, fetchMock, alertMock } = await createDoctorsIndexEnvironment();
      window.showToggleConfirm(5, true, 'Dr. Roe');
      fetchMock.mockResolvedValueOnce({ ok: false, json: async () => ({ success: false }) });

      await window.confirmToggle();

      expect(alertMock).toHaveBeenCalledWith('Failed to update doctor status');
    });

    it('alerts the user when the toggle request throws', async () => {
      const { window, fetchMock, alertMock } = await createDoctorsIndexEnvironment();
      window.showToggleConfirm(5, true, 'Dr. Roe');
      fetchMock.mockRejectedValueOnce(new Error('network down'));

      await window.confirmToggle();

      expect(alertMock).toHaveBeenCalledWith('Something went wrong. Please try again.');
    });
  });
});