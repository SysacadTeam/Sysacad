// Función para crear una cookie
function setCookie(key, value, expires) {
    let expiresString = "";
    if (expires) {
        expiresString = "; expires=" + expires;
    }
    document.cookie = key + "=" + (value || "") + expiresString + "; path=/";
}

// Función para obtener el valor de una cookie
function getCookie(key) {
    const nameEQ = key + "=";
    const cookies = document.cookie.split(';');
    for (let i = 0; i < cookies.length; i++) {
        let cookie = cookies[i];
        while (cookie.charAt(0) === ' ') cookie = cookie.substring(1, cookie.length);
        if (cookie.indexOf(nameEQ) === 0) {
            return cookie.substring(nameEQ.length, cookie.length);
        }
    }
    return null;
}

// Función para borrar una cookie
function deleteCookie(name) {
    document.cookie = name + "=; Max-Age=-99999999; path=/";
}