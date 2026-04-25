// Close dropdown when clicking outside
document.addEventListener('click', function(event) {
    const avatarContainer = event.target.closest('.user-avatar-container');
    const dropdowns = document.querySelectorAll('.user-dropdown');
    
    if (!avatarContainer) {
        dropdowns.forEach(dropdown => {
            dropdown.style.display = 'none';
        });
    }
});
