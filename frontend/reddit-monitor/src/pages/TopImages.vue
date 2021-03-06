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
                <div class="uk-text-lead">Images were found by RedditCollector</div>
                <div class="uk-text-meta">Last updated at {{lastUpdatedAt}}</div>
            </div>
        </div>
        <image-list :images="images" title="Top images by their url"></image-list>
    </div>
</template>

<script>
import axios from 'axios';
import BaseUrl from './../BaseUrl.js'
import ImageList from './../components/ImageList.vue'

export default {
    name: 'top-images',
    components: {
        ImageList
    },
    data() {
        return {
            images: [],
            total: 0,
            ignored: 0,
            showingIgnored: false,
            imageStats: {images: 0, seconds: 0, lastUpdated: null},
            fetchInterval: 0,
            selectedSubreddits: []
        }
    },
    created() {
        this.selectedSubreddits = this.$subredditStore.getSubreddits();
        this.$subredditStore.subscribeOnSubredditsChanged((s) => {
            this.selectedSubreddits = s;
            this.reload();
        });

        const fetchImagesData = () => this.reload();

        const fetchCollectionData = () => axios.get(`${BaseUrl.Value}/api/RedditPullStats`)
            .then(d => {
                this.imageStats = d.data;
            });

        fetchImagesData();
        fetchCollectionData();

        this.fetchInterval = setInterval(() => {
            fetchImagesData();
            fetchCollectionData();
        }, 35 * 1000);
    },
    destroyed() {
        clearTimeout(this.fetchInterval);
    },
    methods: {
        clickComments(item) {
            item.open = !item.open;
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
            axios.post(`api/TopImages`, {ignored: this.showingIgnored, subreddits: this.selectedSubreddits})
                .then((d) => {
                    const {images, total, ignored} = d.data;
                    this.images = images;
                    this.total = total;
                    this.ignored = ignored;
                });
        }
    },
    computed: {
        lastUpdatedAt() { 
            if(!this.imageStats.lastUpdated) {
                return 'never';
            }

            const date = new Date(this.imageStats.lastUpdated);
            return date.toLocaleString();
        }
    }
}
</script>