export function setTheme(themeName) {
    const newLink = document.createElement("link");
    newLink.setAttribute("id", "theme");
    newLink.setAttribute("rel", "stylesheet");
    newLink.setAttribute("type", "text/css");
    newLink.setAttribute("href", `css/app-${themeName}.css`);

    //remove the theme from the head tag
    const head = document.getElementsByTagName("head")[0];
    head.querySelector("#theme").remove();

    head.appendChild(newLink);
}