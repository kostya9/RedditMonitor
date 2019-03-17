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
            <div class="uk-card uk-padding-large">
                <h1 class="uk-heading-primary">{{imageStats.images}} in {{imageStats.seconds}}s</h1>
                <span class="uk-text-lead">Images were found by RedditCollector</span>
            </div>
        </div>
        <div class="uk-text-center">
            <h1 class="uk-heading-primary">
                Top images by their url
            </h1>
        </div>
        <div uk-grid class="uk-gird-match uk-grid-divider">
            <top-image v-for="(item, index) in images" :image="item" :key="index" @ignore="() => ignore(item)" :showingIgnored="showingIgnored"> </top-image>
        </div>
    </div>
</template>

<script>
import axios from 'axios';

import BaseUrl from './../BaseUrl.js'
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
            showingIgnored: false,
            imageStats: {images: 0, seconds: 0}
        }
    },
    created() {
        const fetchImagesData = () => axios.get(`${BaseUrl.Value}/api/TopImages?ignored=${this.showingIgnored}`)
            .then((d) => {
                const {images, total, ignored} = d.data;
                this.images = images;
                this.total = total;
                this.ignored = ignored;
            });

        const fetchCollectionData = () => axios.get(`${BaseUrl.Value}/api/RedditPullStats`)
            .then(d => {
                this.imageStats = d.data;
            });

        fetchImagesData();
        fetchCollectionData();

        setInterval(() => {
            fetchImagesData();
            fetchCollectionData();
        }, 35 * 1000);
    },
    methods: {
        clickComments(item) {
            item.open = !item.open;
        },
        ignore(item) {
            const basePath = BaseUrl.Value;
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
            const basePath = BaseUrl.Value;
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