// Gallery lightbox: any link with [data-lightbox] opens its image in the <dialog>.
(() => {
    const dialog = document.querySelector("dialog.lightbox");
    if (!dialog) return;
    const img = dialog.querySelector("img");
    const caption = dialog.querySelector(".lightbox-caption");

    document.addEventListener("click", e => {
        const link = e.target.closest("a[data-lightbox]");
        if (!link) return;
        e.preventDefault();
        img.src = link.href;
        img.alt = link.querySelector("img")?.alt ?? "";
        caption.textContent = link.dataset.caption ?? "";
        dialog.showModal();
    });

    // Clicking the backdrop closes it too.
    dialog.addEventListener("click", e => { if (e.target === dialog) dialog.close(); });
})();
