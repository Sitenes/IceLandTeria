(() => {
    "use strict";

    const sidebar = document.getElementById("adminSidebar");
    const menuButton = document.getElementById("adminMenuButton");
    const closeButton = document.getElementById("adminSidebarClose");
    const overlay = document.getElementById("adminOverlay");

    if (!sidebar || !menuButton || !closeButton || !overlay) {
        return;
    }

    const openMenu = () => {
        sidebar.classList.add("is-open");
        overlay.classList.add("is-visible");
        document.body.classList.add("admin-menu-open");

        menuButton.setAttribute("aria-expanded", "true");
        overlay.setAttribute("aria-hidden", "false");
    };

    const closeMenu = () => {
        sidebar.classList.remove("is-open");
        overlay.classList.remove("is-visible");
        document.body.classList.remove("admin-menu-open");

        menuButton.setAttribute("aria-expanded", "false");
        overlay.setAttribute("aria-hidden", "true");
    };

    menuButton.addEventListener("click", () => {
        const isOpen = sidebar.classList.contains("is-open");

        if (isOpen) {
            closeMenu();
        } else {
            openMenu();
        }
    });

    closeButton.addEventListener("click", closeMenu);

    overlay.addEventListener("click", closeMenu);

    document.addEventListener("keydown", (event) => {
        if (event.key === "Escape") {
            closeMenu();
        }
    });

    window.addEventListener("resize", () => {
        if (window.innerWidth > 760) {
            closeMenu();
        }
    });

    const navigationLinks = sidebar.querySelectorAll(
        ".admin-navigation__item"
    );

    navigationLinks.forEach((link) => {
        link.addEventListener("click", () => {
            if (window.innerWidth <= 760) {
                closeMenu();
            }
        });
    });
})();

(() => {
    "use strict";

    const modal = document.getElementById("deleteCategoryModal");

    if (!modal) {
        return;
    }

    const message = document.getElementById(
        "deleteCategoryMessage"
    );

    const deleteForm = document.getElementById(
        "deleteCategoryForm"
    );

    const deleteButtons = document.querySelectorAll(
        "[data-delete-category]"
    );

    const closeButtons = modal.querySelectorAll(
        "[data-close-delete-modal]"
    );

    let lastFocusedElement = null;

    const openModal = (button) => {
        const categoryId = button.dataset.categoryId;
        const categoryTitle = button.dataset.categoryTitle;

        if (!categoryId || !categoryTitle) {
            return;
        }

        lastFocusedElement = document.activeElement;

        message.textContent =
            `آیا از حذف دسته‌بندی «${categoryTitle}» مطمئن هستید؟`;

        deleteForm.action =
            `/Admin/Categories/Delete/${categoryId}`;

        modal.classList.add("is-open");

        modal.setAttribute("aria-hidden", "false");

        document.body.classList.add("admin-menu-open");

        const cancelButton = modal.querySelector(
            "[data-close-delete-modal]"
        );

        if (cancelButton) {
            window.setTimeout(() => {
                cancelButton.focus();
            }, 50);
        }
    };

    const closeModal = () => {
        modal.classList.remove("is-open");

        modal.setAttribute("aria-hidden", "true");

        document.body.classList.remove("admin-menu-open");

        if (lastFocusedElement) {
            lastFocusedElement.focus();
        }
    };

    deleteButtons.forEach((button) => {
        button.addEventListener("click", () => {
            openModal(button);
        });
    });

    closeButtons.forEach((button) => {
        button.addEventListener("click", closeModal);
    });

    document.addEventListener("keydown", (event) => {
        if (
            event.key === "Escape" &&
            modal.classList.contains("is-open")
        ) {
            closeModal();
        }
    });
})();