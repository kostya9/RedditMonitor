<template>
    <div>
        <h1>Target Subreddits</h1>
        <h3>Will use ALL if none are selected</h3>

        <label><input @input="(e) => changeFilterDebounce(e)"> Find subreddit</label>
        <div>
            <vk-button @click="() => selectAll()">Select All</vk-button>
        </div>
        <div>
            <vk-button @click="() => clearAll()">Clear All</vk-button>
        </div>

        <DynamicScroller
            class="scroller"
            :items="filteredSubreddits"
            :min-item-size="32"
            key-field="name">
            <template v-slot="{ item, index, active }">
                <DynamicScrollerItem        
                    :item="item"
                    :active="active"
                    :size-dependencies="[
                        item.name,
                    ]"
                    :data-index="index">
                    <label><input class="uk-checkbox" type="checkbox" @change="() => changeCheckbox()" v-model="item.value"> {{item.name}} </label>
                </DynamicScrollerItem>
            </template>
        </DynamicScroller>
    </div>
</template>

<script>
import axios from 'axios';
import _ from 'lodash';

export default {
    data() {
        return {
            subreddits: [],
            subredditFilter: ""
        }
    },
    mounted() {
        axios.get('api/subreddits')
        .then((r) => {
            this.subreddits = r.data
                .sort((a, b) => a.toLowerCase().localeCompare(b.toLowerCase()))
                .map(s => ({name: s, value: false}));
        });
    },
    methods: {
        changeCheckbox() {
            this.$subredditStore.setSubreddits(this.subreddits.filter(s => s.value).map(s => s.name));
        },
        clearAll() {
            for(var s of this.filteredSubreddits) {
                s.value = false;
            }
            this.$subredditStore.setSubreddits(this.subreddits.filter(s => s.value).map(s => s.name));
        },
        selectAll() {
            for(var s of this.filteredSubreddits) {
                s.value = true;
            }
            this.$subredditStore.setSubreddits(this.subreddits.filter(s => s.value).map(s => s.name));
        },
        changeFilterDebounce:  _.debounce(function(e) { this.changeFilter(e)}, 200),
        changeFilter(e) {
            this.subredditFilter = e.target.value;
        }
    },
    computed: {
        filteredSubreddits() { 
            if(this.subredditFilter && this.subredditFilter.length) {
                var filter = this.subredditFilter.toLowerCase();
                return this.subreddits.filter(s => s.name.toLowerCase().includes(filter));
            }

            return this.subreddits;
        }
    }
        
}
</script>

<style>
.scroller {
  height: 500px;
  overflow-y: auto;
}

.scroller > label {
  height: 32%;
  padding: 0 12px;
  display: flex;
  align-items: center;
}
</style>
