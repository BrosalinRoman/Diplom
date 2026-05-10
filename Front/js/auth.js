document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('loginForm');
    const togglePassword = document.querySelector('.toggle-password');
    const passwordInput = document.getElementById('password');

    if (togglePassword && passwordInput) {
        togglePassword.addEventListener('click', () => {
            const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
            passwordInput.setAttribute('type', type);
            togglePassword.textContent = type === 'password' ? '👁️' : '👁️‍🗨️';
        });
    }

    if (loginForm) {
        loginForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            const username = document.getElementById('username').value;
            const password = document.getElementById('password').value;

            if (!username || !password) {
                showNotification('Заполните все поля', 'error');
                return;
            }

            try {
                showNotification('Выполняется вход...', 'info');
                const data = await window.api.login({ username, password });
                localStorage.setItem('token', data.accessToken);
                localStorage.setItem('user', JSON.stringify({
                    userId: data.userId,
                    role: data.role,
                    fullName: data.fullName,
                    login: username
                }));
                showNotification('Вход выполнен успешно!', 'success');
                setTimeout(() => {
                    window.location.href = 'home.html';
                }, 100);
            } catch (err) {
                showNotification(err.message || 'Ошибка входа', 'error');
            }
        });
    }
});