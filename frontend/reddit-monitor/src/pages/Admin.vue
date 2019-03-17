<template>
    <div>
        <div class="uk-flex uk-flex-center uk-text-center">
            <div class="uk-card uk-padding-large">
                <h1 class="uk-heading-primary">{{total}}</h1>
                <a class="uk-text-lead" @click="() => notShowIgnored()">Total images {{!showingIgnored ? '(showing)' : ''}}</a>
            </div>
            <div class="uk-card uk-padding-large">
                <h1 class="uk-heading-primary">{{ignored}}</h1>
                <a class="uk-text-lead uk-link" @click="() => showIgnored()">Ignored images {{showingIgnored ? '(showing)' : ''}}</a>
            </div>
        </div>
        <div uk-grid class="uk-gird-match uk-grid-divider">
            <top-image v-for="(item, index) in images" :image="item" :key="index" @ignore="() => ignore(item)" :showingIgnored="showingIgnored"> </top-image>
        </div>
    </div>
</template>

<script>
import axios from 'axios';

import TopImage from './../components/Admin/TopImage.vue'

export default {
    name: 'admin',
    components: {
        TopImage
    },
    data() {
        return {
            images: [],
            total: 0,
            ignored: 0,
            showingIgnored: false
        }
    },
    mounted() {
        const basePath = '';
        //const basePath = 'http://localhost:64621';
        axios.get(`${basePath}/api/TopImages?ignored=${this.showingIgnored}`)
            .then((d) => {
                const {images, total, ignored} = d.data;
                this.images = images;
                this.total = total;
                this.ignored = ignored;
            });
    },
    methods: {
        clickComments(item) {
            item.open = !item.open;
        },
        ignore(item) {
            //const basePath = 'http://localhost:64621';
            const basePath = '';
            axios.post(`${basePath}/api/TopImages/ignore`, {value: !this.showingIgnored, imageUrl: item.url})
                .then(() => axios.get(`${basePath}/api/TopImages?ignored=${this.showingIgnored}`))
                .then((d) => {
                    const {images, total, ignored} = d.data;
                    this.images = images;
                    this.total = total;
                    this.ignored = ignored;
                });
        },
        showIgnored() {
            this.showingIgnored = true;
            this.reload();
        },
        notShowIgnored() {
            this.showingIgnored = false;
            this.reload();
        },
        reload() {
            const basePath = '';
            //const basePath = 'http://localhost:64621';
            axios.get(`${basePath}/api/TopImages?ignored=${this.showingIgnored}`)
                .then((d) => {
                    const {images, total, ignored} = d.data;
                    this.images = images;
                    this.total = total;
                    this.ignored = ignored;
                });
        }
    },
}
</script>