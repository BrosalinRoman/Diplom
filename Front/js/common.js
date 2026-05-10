// ======================== ОБЩИЕ УТИЛИТЫ ========================

function showNotification(message, type = 'info') {
    const existing = document.querySelector('.notification');
    if (existing) existing.remove();
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.textContent = message;
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 1rem 1.5rem;
        border-radius: 4px;
        color: white;
        font-weight: 500;
        z-index: 10000;
        animation: slideIn 0.3s ease;
        background-color: ${type === 'error' ? '#e74c3c' : type === 'success' ? '#27ae60' : '#3498db'};
    `;
    document.body.appendChild(notification);
    setTimeout(() => {
        notification.style.animation = 'slideOut 0.5s ease';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

function getCurrentUser() {
    const userStr = localStorage.getItem('user');
    if (!userStr) return null;
    try { return JSON.parse(userStr); } catch(e) { return null; }
}

function getUserRole() {
    const user = getCurrentUser();
    return user ? user.role : null;
}

function getUserId() {
    const user = getCurrentUser();
    return user ? user.userId : null;
}

function formatCurrency(value) {
    if (value === undefined || value === null) return '0,00';
    return value.toLocaleString('ru-RU', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

function formatDate(isoString) {
    if (!isoString) return '';
    const date = new Date(isoString);
    return date.toLocaleDateString('ru-RU');
}

function formatShortName(fullName) {
    if (!fullName) return 'Пользователь';
    const parts = fullName.trim().split(/\s+/);
    if (parts.length < 2) return parts[0];
    let lastName = parts[0];
    let firstName = parts[1] ? parts[1][0] + '.' : '';
    let patronymic = parts[2] ? parts[2][0] + '.' : '';
    if (lastName.length > 10) {
        lastName = lastName.slice(0, 7) + '...';
    }
    return `${lastName} ${firstName}${patronymic}`.trim();
}

// Модальное окно подтверждения (замена alert)
function confirmDialog(message, onConfirm) {
    let modal = document.getElementById('confirmModal');
    if (!modal) {
        modal = document.createElement('div');
        modal.id = 'confirmModal';
        modal.className = 'modal-overlay';
        modal.innerHTML = `
            <div class="modal">
                <p id="confirmMessage" style="margin-bottom: 1.5rem;">${message}</p>
                <div class="modal-actions">
                    <button class="btn-secondary" id="confirmNo">Отмена</button>
                    <button class="btn-primary" id="confirmYes">Да</button>
                </div>
            </div>
        `;
        document.body.appendChild(modal);
    }
    document.getElementById('confirmMessage').innerText = message;
    modal.classList.add('active');
    const onYes = () => {
        modal.classList.remove('active');
        if (onConfirm) onConfirm();
        cleanup();
    };
    const onNo = () => {
        modal.classList.remove('active');
        cleanup();
    };
    const cleanup = () => {
        document.getElementById('confirmYes').removeEventListener('click', onYes);
        document.getElementById('confirmNo').removeEventListener('click', onNo);
    };
    document.getElementById('confirmYes').addEventListener('click', onYes);
    document.getElementById('confirmNo').addEventListener('click', onNo);
}

function initHeader() {
    const user = getCurrentUser();
    if (user) {
        const userNameSpan = document.querySelector('.user-name');
        if (userNameSpan) {
            const shortName = formatShortName(user.fullName || user.login);
            const roleMap = { 'Admin': 'Администратор', 'Investor': 'Инвестор', 'Applicant': 'Заявитель' };
            const roleRu = roleMap[user.role] || user.role;
            userNameSpan.innerHTML = `${shortName}<br/>(${roleRu})`;
        }
    }
    const logoutBtn = document.querySelector('.logout-btn');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', async () => {
            confirmDialog('Вы уверены, что хотите выйти?', async () => {
                await window.api.logout();
                window.location.href = 'index.html';
            });
        });
    }
}

function checkAuth() {
    const token = localStorage.getItem('token');
    if (!token && window.location.pathname !== '/index.html') {
        window.location.href = 'index.html';
        return false;
    }
    return true;
}

window.showNotification = showNotification;
window.getCurrentUser = getCurrentUser;
window.getUserRole = getUserRole;
window.getUserId = getUserId;
window.formatCurrency = formatCurrency;
window.formatDate = formatDate;
window.formatShortName = formatShortName;
window.confirmDialog = confirmDialog;
window.initHeader = initHeader;
window.checkAuth = checkAuth;