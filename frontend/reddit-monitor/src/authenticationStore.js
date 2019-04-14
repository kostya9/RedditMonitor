import Vue from 'vue'

export default class AuthenticationStore extends Vue {
    constructor() {
        super();

        this.tokenKey = 'token';
        this.initToken();
    }

    initToken() {
        const localStorageToken = localStorage[this.tokenKey];
        if(localStorageToken) {
            this.token = localStorageToken;
        }
        else {
            this.token = null;
        }
    }

    isSignedin() {
        return !!this.token;
    }

    getToken() {
        return this.token;
    }

    signin(token) {
        this.token = token;
        localStorage[this.tokenKey] = token;
        this.$emit('signin');
    }

    signout() {
        this.token = null;
        localStorage.removeItem(this.tokenKey);
        this.$emit('signout');
    }

    subscribeSignin(f) {
        this.$on('signin', f);
    }

    subscribeSignout(f) {
        this.$on('signout', f);
    }
}