window.cookieInterop = {
    set: function (name, value, expires) {
        if (!name) {
            return;
        }

        var cookie = name + '=' + value + '; path=/; SameSite=Lax';
        if (expires) {
            cookie += '; expires=' + expires;
        }

        if (window && window.location && window.location.protocol === 'https:') {
            cookie += '; Secure';
        }

        document.cookie = cookie;
    },
    get: function (name) {
        if (!name) {
            return null;
        }

        var pattern = new RegExp('(?:^|; )' + name.replace(/([.$?*|{}()\[\]\\\/\+^])/g, '\\$1') + '=([^;]*)');
        var matches = document.cookie.match(pattern);
        return matches ? matches[1] : null;
    },
    delete: function (name) {
        if (!name) {
            return;
        }

        document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/; SameSite=Lax';
    }
};
