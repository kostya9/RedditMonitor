import Vue from 'vue';

export default class SelectedSubredditStore extends Vue {
    constructor() {
        super();
        this.subreddits = [];
    }

    getSubreddits() {
        return this.subreddits;
    }

    subscribeOnSubredditsChanged(func) {
        this.$on('subredditschanged', func);
    }

    setSubreddits(subreddits) {
        this.subreddits = subreddits;
        this.$emit('subredditschanged', subreddits);
    }
}