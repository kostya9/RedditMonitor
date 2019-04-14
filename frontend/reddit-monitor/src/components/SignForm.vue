<template>
    <div>
    <vk-tabs align="center">
        <vk-tabs-item title="Sign In">
            <div class="signin" @keyup="(e) => e.keyCode == 13 && signin()">
                <div class="uk-margin uk-text-center">
                    <div class="uk-inline uk-width-1-2">
                        <span class="uk-form-icon"><vk-icons-user></vk-icons-user></span>
                        <input class="uk-input" type="text" v-model="signinUsername">
                    </div>
                </div>
                <div class="uk-margin uk-text-center">
                    <div class="uk-inline uk-width-1-2">
                        <span class="uk-form-icon"><vk-icons-lock></vk-icons-lock></span>
                        <input class="uk-input" type="password" v-model="signinPassword">
                    </div>
                </div>
                <div class="uk-margin uk-text-center">
                    <vk-button type="primary" class="uk-inline uk-width-1-2" :disabled="signinDisabled" @click="signin">LOGIN</vk-button>
                </div>
            </div>
        </vk-tabs-item>
        <vk-tabs-item title="Sign Up">
            <form class="uk-form-horizontal" @keyup="(e) => e.keyCode == 13 && signup()" v-if="!confirmEmail">

                <div class="uk-margin">
                    <label class="uk-form-label" for="username">Username</label>
                    <div class="uk-form-controls">
                        <input class="uk-input" id="username" type="text" placeholder="Username" v-model="signupUsername">
                    </div>
                </div>

                <div class="uk-margin">
                    <label class="uk-form-label" for="email">Email address</label>
                    <div class="uk-form-controls">
                        <input class="uk-input" id="email" type="text" placeholder="you@company.com" v-model="signupEmail">
                    </div>
                </div>


                <div class="uk-margin">
                    <label class="uk-form-label" for="pwd">Password</label>
                    <div class="uk-form-controls">
                        <input class="uk-input" id="pwd" type="password" placeholder="Password" v-model="signupPassword">
                    </div>
                </div>

                <div class="uk-margin">
                    <label class="uk-form-label" for="confirm">Password confirmation</label>
                    <div class="uk-form-controls">
                        <input class="uk-input" id="confirm" type="password" placeholder="Password" v-model="signupConfirmPassword">
                    </div>
                </div>

                <div class="uk-margin uk-text-center">
                    <vk-button type="primary" class="uk-inline uk-width-1-2" :disabled="signupDisabled" @click="signup">SUBMIT</vk-button>
                </div>

            </form>
            <span v-if="confirmEmail">
                Please confirm your email {{signupEmail}} via clicking the link in confirmation email <a @click="confirmEmail = false"> or try again </a>
            </span>
        </vk-tabs-item>
  </vk-tabs>
  </div>
</template>

<script>
import Axios from 'axios';

export default {
    data: function () {
        return {
            signinUsername: "",
            signinPassword: "",
            signupUsername: "",
            signupPassword: "",
            signupConfirmPassword: "",
            signupEmail: "",
            confirmEmail: false
        }
    },
    methods: {
        signin() {
            if (this.signinDisabled)
                return;

            Axios.post('/api/users/signin', {username: this.signinUsername, password: this.signinPassword})
                .then((r) => {
                    this.$auth.signin(r.data.token);
                })
                .catch(error => {
                    this.$notifications.error(error.response.data);
                });
        },
        signup() {
            if (this.signupDisabled)
                return;

            Axios.post('/api/users/signup', {username: this.signupUsername, password: this.signupPassword, confirmPassword: this.signupConfirmPassword, email: this.signupEmail})
                .then(() => {
                    this.$notifications.success('Registration successful! Please confirm your account by clicking on the link in confirmation email');
                    this.confirmEmail = true;
                })
                .catch(error => {
                    this.$notifications.error(error.response.data);
                });
        }
    },
    computed: {
        signinDisabled() {
            var validUsername = this.signinUsername && this.signinUsername.length;
            var validPassword = this.signinPassword && this.signinPassword.length;
            return !(validUsername && validPassword);
        },
        signupDisabled() {
            var validUsername = this.signupUsername && this.signupUsername.length;
            var validPassword = this.signupPassword && this.signupPassword.length;
            var validConfirmPassword = this.signupConfirmPassword && this.signupConfirmPassword.length;
            var validEmail = this.signupPassword && this.signupPassword.length;
            return !(validUsername && validPassword && validConfirmPassword && validEmail);
        }
    }
}
</script>

<style>

</style>
