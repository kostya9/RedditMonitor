<template>
    <div>
        <div class="uk-flex uk-flex-center uk-text-center">
            <div class="uk-card uk-padding-large">
                <h1 class="uk-heading-primary">{{total}}</h1>
                <div class="uk-text-lead">Total images</div>
            </div>
            <div class="uk-card uk-padding-large">
                <h1 class="uk-heading-primary">{{ignored}}</h1>
                <div class="uk-text-lead">Ignored images</div>
            </div>
        </div>
        <div uk-grid class="uk-gird-match uk-grid-divider">
            <top-image v-for="(item, index) in images" :image="item" :key="index"> </top-image>
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
            showIgnored: false
        }
    },
    mounted() {
        //const basePath = '';
        const basePath = 'http://localhost:64621';
        axios.get(`${basePath}/api/TopImages?ignored=${this.showIgnored}`)
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
        }
    },
}
</script>