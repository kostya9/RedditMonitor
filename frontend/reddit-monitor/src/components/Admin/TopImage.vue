<template>
        <div class="uk-card uk-card-default uk-card-hover uk-width-1-4">
        <div class="uk-card-header">
            <img :src="image.url">
        </div>
        <div class="uk-card-body">
            <div class="uk-text-lead uk-margin-bottom">
                Count: {{image.count}}
            </div>
            <button class="uk-button uk-button-default" @click="() => clickComments()">
                Show posts {{areLinksOpen ? '-' : '+' }}
            </button>
            <div v-if="areLinksOpen">
                <div v-for="(item, index) in image.comments" :key="index" :href="item">
                    <a class="uk-link-muted" :href="item" target='_blank'>{{item}}</a>
                </div>
            </div>
        </div>
        <div class="uk-card-footer">
            <a class="uk-button uk-button-secondary" target='_blank' :href="image.url">
                Go To Image
            </a>
            <button class="uk-button uk-button-danger" @click="() => $emit('ignore', !showingIgnored)">
                {{showingIgnored ? 'Unignore' : 'Ignore'}}
            </button>
        </div>
    </div>
</template>

<script>
export default {
    name: 'top-image',
    props: ['image', 'showingIgnored'],
    data() {
        return {
            areLinksOpen: false
        }
    },
    methods: {
        clickComments() {
            this.areLinksOpen = !this.areLinksOpen;
        }
    },
}
</script>

<style lang="css" scoped>
    a {
        word-wrap: break-word;
    }
</style>