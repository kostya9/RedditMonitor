<template>
    <div>
        <div v-for="subreddit in subreddits" :key="subreddit">
            <label><input  class="uk-checkbox" type="checkbox" @change="(e) => changeCheckbox(e, subreddit)"> {{subreddit}} </label>
        </div>
    </div>
</template>

<script>
import axios from 'axios';
export default {
    data() {
        return {
            subreddits: [],
            selected: []
        }
    },
    mounted() {
        axios.get('api/subreddits', (r) => {
            this.subreddits = r.data;
        })
    }

}
</script>

<style>

</style>
