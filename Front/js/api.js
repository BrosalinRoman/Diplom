// ======================== КОНФИГУРАЦИЯ ========================
const MOCK_COLLEAGUE_BACKEND = true;  // только для JWT (логин)
const MY_API_BASE = 'https://localhost:7279/api';

// ======================== ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ ========================
function toQueryString(params) {
    const searchParams = new URLSearchParams();
    Object.entries(params).forEach(([key, value]) => {
        if (value === undefined || value === null || value === '') return;
        if (Array.isArray(value)) {
            value.forEach(v => searchParams.append(key, v));
        } else {
            searchParams.append(key, value);
        }
    });
    return searchParams.toString() ? '?' + searchParams.toString() : '';
}

async function myRequest(endpoint, { method = 'GET', body = null, params = {} } = {}) {
    const token = localStorage.getItem('token');
    const headers = { 'Content-Type': 'application/json' };
    if (token) headers['Authorization'] = `Bearer ${token}`;

    const url = `${MY_API_BASE}${endpoint}${toQueryString(params)}`;
    const response = await fetch(url, { method, headers, body: body ? JSON.stringify(body) : undefined });

    if (response.status === 401) {
        localStorage.clear();
        window.location.href = 'index.html';
        throw new Error('Ошибка 401: Сессия истекла, войдите заново');
    }
    if (!response.ok) {
        let errorMsg = `Ошибка ${response.status}`;
        try {
            const errorData = await response.json();
            errorMsg = `Ошибка ${response.status}: ${errorData.Error || errorData.message || 'Неизвестная ошибка'}`;
        } catch (e) {
            errorMsg = `Ошибка ${response.status}: Не удалось получить детали`;
        }
        throw new Error(errorMsg);
    }
    if (response.status === 204) return null;
    return response.json();
}

// ======================== МОК ДЛЯ БЭКЕНДА КОЛЛЕГИ (только JWT) ========================
const mockUsers = [
    { id: 1, login: 'ivanov', password: 'password', role: 'Admin', fullName: 'Иванов Иван Иванович',
      token: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJpdmFub3YiLCJ1c2VySWQiOiIxIiwicm9sZXMiOlsiQWRtaW4iXSwiaWF0IjoxNzc1NjU2NjMyMzU1LCJleHAiOjI3NzU2NTY2MzIzNTV9.i_dh-e0awv5sCnXNX2zOH4s4mXWI_4gvRhLMm-ueU8I' },
    { id: 2, login: 'petrov', password: 'password', role: 'Applicant', fullName: 'Петров Пётр Петрович',
      token: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJwZXRyb3YiLCJ1c2VySWQiOiIyIiwicm9sZXMiOlsiQXBwbGljYW50Il0sImlhdCI6MTc3NTY1NjYzMjM1NSwiZXhwIjoyNzc1NjU2NjMyMzU1fQ.7IZbOuHSD8CkMEZTBVn3_eIzCyQ9kjh8DjcdrL93-jU' },
    { id: 3, login: 'sidorov', password: 'password', role: 'Investor', fullName: 'Сидоров Сидор Сидорович',
      token: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzaWRvcm92IiwidXNlcklkIjoiMyIsInJvbGVzIjpbIkludmVzdG9yIl0sImlhdCI6MTc3NTY1NjYzMjM1NSwiZXhwIjoyNzc1NjU2NjMyMzU1fQ.PZQFhZhTTRl5VsIAXcRbWas3ctcQuhFIdhB9zIkVmsM' },
    { id: 4, login: 'kuznetsov', password: 'password', role: 'Applicant', fullName: 'Кузнецов Алексей Владимирович',
      token: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJrdXpuZXRzb3YiLCJ1c2VySWQiOiI0Iiwicm9sZXMiOlsiQXBwbGljYW50Il0sImlhdCI6MTc3NTY1NjYzMjM1NSwiZXhwIjoyNzc1NjU2NjMyMzU1fQ.c0y-uZVV3A3bRXDSX2sBWpVMFPffMdWjxlWhKn--FYY' }
];
async function mockLogin(credentials) {
    const { username, password } = credentials;
    const user = mockUsers.find(u => u.login === username && u.password === password);
    if (!user) throw new Error('Неверный логин или пароль');
    return { accessToken: user.token, userId: user.id, role: user.role, fullName: user.fullName };
}

// ======================== РЕАЛЬНЫЕ ФУНКЦИИ ДЛЯ ВАШЕГО БЭКЕНДА ========================
async function getCategories() { return myRequest('/Nsi/categories'); }
async function getDirections() { return myRequest('/Nsi/directions'); }
async function getDepartments() { return myRequest('/Nsi/departments'); }
async function getStatuses() { return myRequest('/Nsi/statuses'); }

// Аналитика с пагинацией
async function getProjectsAnalytics(filters) {
    return myRequest('/analytics/projects', { params: filters });
}
async function getSummaryByDepartments(filters) {
    return myRequest('/analytics/summary/departments', { params: filters });
}
// Контроль с пагинацией
async function getControlProjects(filters) {
    return myRequest('/control/projects', { params: filters });
}
async function getProjectInfo(projectId) {
    return myRequest(`/control/projects/${projectId}/info`);
}
// Инвестиции, затраты, отчёты
async function getInvestments(projectId) { return myRequest(`/control/projects/${projectId}/investments`); }
async function addInvestment(projectId, data) { return myRequest(`/control/projects/${projectId}/investments`, { method: 'POST', body: data }); }
async function updateInvestment(id, data) { return myRequest(`/control/investments/${id}`, { method: 'PUT', body: data }); }
async function deleteInvestment(id) { return myRequest(`/control/investments/${id}`, { method: 'DELETE' }); }
async function getCosts(projectId) { return myRequest(`/control/projects/${projectId}/costs`); }
async function addCost(projectId, data) { return myRequest(`/control/projects/${projectId}/costs`, { method: 'POST', body: data }); }
async function updateCost(id, data) { return myRequest(`/control/costs/${id}`, { method: 'PUT', body: data }); }
async function deleteCost(id) { return myRequest(`/control/costs/${id}`, { method: 'DELETE' }); }
async function getProgressReports(projectId) { return myRequest(`/control/projects/${projectId}/progress-reports`); }
async function addProgressReport(projectId, data) { return myRequest(`/control/projects/${projectId}/progress-reports`, { method: 'POST', body: data }); }
async function updateProgressReport(id, data) { return myRequest(`/control/progress-reports/${id}`, { method: 'PUT', body: data }); }
async function deleteProgressReport(id) { return myRequest(`/control/progress-reports/${id}`, { method: 'DELETE' }); }

// Шаблоны
async function getTemplates() { return myRequest('/analytics/templates'); }
async function getTemplateById(id) { return myRequest(`/analytics/templates/${id}`); }
async function createTemplate(data) { return myRequest('/analytics/templates', { method: 'POST', body: data }); }
async function updateTemplate(id, data) { return myRequest(`/analytics/templates/${id}`, { method: 'PUT', body: data }); }
async function deleteTemplate(id) { return myRequest(`/analytics/templates/${id}`, { method: 'DELETE' }); }

// Авторизация
async function login(credentials) {
    if (MOCK_COLLEAGUE_BACKEND) return mockLogin(credentials);
    // здесь будет реальный вызов к бэку коллеги
}
async function logout() { localStorage.clear(); }

window.api = {
    login, logout,
    getProjectsAnalytics, getSummaryByDepartments,
    getTemplates, getTemplateById, createTemplate, updateTemplate, deleteTemplate,
    getControlProjects, getProjectInfo,
    getInvestments, addInvestment, updateInvestment, deleteInvestment,
    getCosts, addCost, updateCost, deleteCost,
    getProgressReports, addProgressReport, updateProgressReport, deleteProgressReport,
    getCategories, getDirections, getDepartments, getStatuses
};