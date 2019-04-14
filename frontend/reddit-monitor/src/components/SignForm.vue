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
            <form class="uk-form-horizontal" @keyup="(e) => e.keyCode == 13 && signup()">

                <div class="uk-margin">
                    <label class="uk-form-label" for="username">Username</label>
                    <div class="uk-form-controls">
                        <input class="uk-input" id="username" type="text" placeholder="Username">
                    </div>
                </div>

                <div class="uk-margin">
                    <label class="uk-form-label" for="email">Email address</label>
                    <div class="uk-form-controls">
                        <input class="uk-input" id="email" type="text" placeholder="you@company.com">
                    </div>
                </div>


                <div class="uk-margin">
                    <label class="uk-form-label" for="pwd">Password</label>
                    <div class="uk-form-controls">
                        <input class="uk-input" id="pwd" type="password" placeholder="Password">
                    </div>
                </div>

                <div class="uk-margin">
                    <label class="uk-form-label" for="confirm">Password confirmation</label>
                    <div class="uk-form-controls">
                        <input class="uk-input" id="confirm" type="password" placeholder="Password">
                    </div>
                </div>

                <div class="uk-margin uk-text-center">
                    <vk-button type="primary" class="uk-inline uk-width-1-2">SUBMIT</vk-button>
                </div>

            </form>
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
            signinPassword: ""
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
                    // eslint-disable-next-line
                    console.log(error.response)
                    this.$notifications.error(error.response.data);
                });
        },
        signup() {
            // eslint-disable-next-line
            console.log(2);
        }
    },
    computed: {
        signinDisabled() {
            var validUsername = this.signinUsername && this.signinUsername.length;
            var validPassword = this.signinPassword && this.signinPassword.length;
            return !(validUsername && validPassword);
        }
    }
}
</script>

<style>

</style>
