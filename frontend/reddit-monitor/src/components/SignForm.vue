<template>
    <vk-tabs align="center">
        <vk-tabs-item title="Sign In">
            <div class="signin">
                <div class="uk-margin uk-text-center">
                    <div class="uk-inline uk-width-1-2">
                        <span class="uk-form-icon"><vk-icons-user></vk-icons-user></span>
                        <input class="uk-input" type="text">
                    </div>
                </div>
                <div class="uk-margin uk-text-center">
                    <div class="uk-inline uk-width-1-2">
                        <span class="uk-form-icon"><vk-icons-lock></vk-icons-lock></span>
                        <input class="uk-input" type="password">
                    </div>
                </div>
                <div class="uk-margin uk-text-center">
                    <vk-button type="primary" class="uk-inline uk-width-1-2">LOGIN</vk-button>
                </div>
            </div>
        </vk-tabs-item>
        <vk-tabs-item title="Sign Up">
            <form class="uk-form-horizontal">

                <div class="uk-margin">
                    <label class="uk-form-label" for="form-horizontal-text">Login</label>
                    <div class="uk-form-controls">
                        <input class="uk-input" id="form-horizontal-text" type="text" placeholder="Login">
                    </div>
                </div>

                <div class="uk-margin">
                    <label class="uk-form-label" for="form-horizontal-text">Email address</label>
                    <div class="uk-form-controls">
                        <input class="uk-input" id="form-horizontal-text" type="text" placeholder="you@company.com">
                    </div>
                </div>


                <div class="uk-margin">
                    <label class="uk-form-label" for="form-horizontal-text">Password</label>
                    <div class="uk-form-controls">
                        <input class="uk-input" id="form-horizontal-text" type="password" placeholder="Password">
                    </div>
                </div>

                <div class="uk-margin">
                    <label class="uk-form-label" for="form-horizontal-text">Password confirmation</label>
                    <div class="uk-form-controls">
                        <input class="uk-input" id="form-horizontal-text" type="password" placeholder="Password">
                    </div>
                </div>

                <div class="uk-margin uk-text-center">
                    <vk-button type="primary" class="uk-inline uk-width-1-2">SUBMIT</vk-button>
                </div>

            </form>
        </vk-tabs-item>
  </vk-tabs>
</template>

<script>
export default {
    data: function (){
        return {
            activeTab: 'signin'
        }
    }
}
</script>

<style>

</style>
