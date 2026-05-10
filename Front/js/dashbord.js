document.addEventListener('DOMContentLoaded', function() {
    // Обработка выхода из системы
    const logoutBtn = document.querySelector('.logout-btn');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', function() {
            if (confirm('Вы уверены, что хотите выйти из системы?')) {
                // В реальном приложении здесь был бы запрос на сервер для выхода
                window.location.href = 'index.html';
            }
        });
    }
    
    // Анимация карточек при загрузке
    const featureCards = document.querySelectorAll('.feature-card');
    featureCards.forEach((card, index) => {
        card.style.animationDelay = `${index * 0.1}s`;
        card.classList.add('animate-in');
    });
    
    // Добавляем стили для анимации
    const style = document.createElement('style');
    style.textContent = `
        .feature-card.animate-in {
            animation: slideUp 0.5s ease forwards;
            opacity: 0;
            transform: translateY(20px);
        }
        @keyframes slideUp {
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }
    `;
    document.head.appendChild(style);
    
    // Обработка быстрых ссылок
    const cardLinks = document.querySelectorAll('.card-link');
    cardLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const targetUrl = this.getAttribute('href');
            
            // Добавляем анимацию перехода
            document.body.style.opacity = '0.7';
            document.body.style.transition = 'opacity 0.3s ease';
            
            setTimeout(() => {
                window.location.href = targetUrl;
            }, 300);
        });
    });
});