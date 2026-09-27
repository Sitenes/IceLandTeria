(function () {
    "use strict";

    function normalizePersianNumber(value) {
        if (!value) {
            return "";
        }

        return value
            .replace(/۰/g, "0")
            .replace(/۱/g, "1")
            .replace(/۲/g, "2")
            .replace(/۳/g, "3")
            .replace(/۴/g, "4")
            .replace(/۵/g, "5")
            .replace(/۶/g, "6")
            .replace(/۷/g, "7")
            .replace(/۸/g, "8")
            .replace(/۹/g, "9")
            .replace(/٬/g, "")
            .replace(/،/g, "")
            .replace(/,/g, "")
            .replace(/٫/g, ".");
    }

    document.querySelectorAll(".js-price-input").forEach(function (input) {
        input.addEventListener("blur", function () {
            input.value = normalizePersianNumber(input.value);
        });
    });

    const modal = document.getElementById("deleteModal");
    const productTitleElement =
        document.getElementById("deleteProductTitle");
    const confirmButton =
        document.getElementById("confirmDeleteButton");

    let activeDeleteForm = null;
    let previouslyFocusedElement = null;

    if (!modal || !productTitleElement || !confirmButton) {
        return;
    }

    function openModal(form, productTitle) {
        activeDeleteForm = form;
        previouslyFocusedElement = document.activeElement;

        productTitleElement.textContent =
            "«" + productTitle + "»";

        modal.classList.add("is-open");
        modal.setAttribute("aria-hidden", "false");

        document.body.style.overflow = "hidden";

        confirmButton.focus();
    }

    function closeModal() {
        modal.classList.remove("is-open");
        modal.setAttribute("aria-hidden", "true");

        document.body.style.overflow = "";

        activeDeleteForm = null;

        if (previouslyFocusedElement) {
            previouslyFocusedElement.focus();
        }

        previouslyFocusedElement = null;
    }

    document
        .querySelectorAll(".js-delete-product")
        .forEach(function (button) {
            button.addEventListener("click", function () {
                const form = button.closest(".delete-product-form");

                if (!form) {
                    return;
                }

                const productTitle =
                    button.getAttribute("data-product-title") ||
                    "این محصول";

                openModal(form, productTitle);
            });
        });

    document
        .querySelectorAll(".js-close-delete-modal")
        .forEach(function (element) {
            element.addEventListener("click", closeModal);
        });

    confirmButton.addEventListener("click", function () {
        if (!activeDeleteForm) {
            return;
        }

        activeDeleteForm.submit();
    });

    document.addEventListener("keydown", function (event) {
        if (
            event.key === "Escape" &&
            modal.classList.contains("is-open")
        ) {
            closeModal();
        }
    });

    modal.addEventListener("keydown", function (event) {
        if (event.key !== "Tab") {
            return;
        }

        const focusableElements = modal.querySelectorAll(
            "button:not([disabled]), [href], input:not([disabled]), select:not([disabled]), textarea:not([disabled])"
        );

        if (focusableElements.length === 0) {
            return;
        }

        const firstElement = focusableElements[0];
        const lastElement =
            focusableElements[focusableElements.length - 1];

        if (event.shiftKey && document.activeElement === firstElement) {
            event.preventDefault();
            lastElement.focus();
        } else if (
            !event.shiftKey &&
            document.activeElement === lastElement
        ) {
            event.preventDefault();
            firstElement.focus();
        }
    });
})();