// Closes the mobile menu after a link or button inside it is tapped.
// One delegated listener on the document, so it keeps working after Blazor's
// enhanced navigation replaces the page content.
document.addEventListener('click', (event) => {
    if (!event.target.closest('.main-nav a, .main-nav button, .nav-cta')) {
        return;
    }

    const toggle = document.getElementById('nav-toggle');
    if (toggle) {
        toggle.checked = false;
    }
});
