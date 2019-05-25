<template>
    <div class="uk-card uk-card-default uk-card-hover uk-width-1-6 uk-margin">
        <div class="uk-card-header">
            <img :src="image.url">
        </div>
        <div class="uk-card-body">
            <div class="uk-text-lead uk-margin-bottom">
                Count: {{image.count}}
            </div>
        </div>
        <div class="uk-card-footer">
            <vk-button-link  @click="() => this.$emit('opened', image)">
                Posts +
            </vk-button-link>
        </div>
    </div>
</template>

<script>
export default {
    name: 'top-image',
    props: ['image']
}
</script>

<style lang="css" scoped>
    a {
        word-wrap: break-word;
    }
</style>