document.addEventListener("DOMContentLoaded", function () {

    const loginButton = document.querySelector(".button input");
    loginButton.addEventListener("mouseover", () => {
        loginButton.style.transform = "scale(1.05)"; 
    });
    loginButton.addEventListener("mouseout", () => {
        loginButton.style.transform = "scale(1)"; 
    });

    const inputs = document.querySelectorAll("input");
    inputs.forEach(input => {
        input.addEventListener("focus", () => {
            input.style.borderColor = "#00bfa5"; 
        });
        input.addEventListener("blur", () => {
            input.style.borderColor = "#ccc"; 
        });
    });
});
