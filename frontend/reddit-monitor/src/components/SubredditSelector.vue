<template>
    <div>
        <h1 class="uk-heading-line uk-text-center"><span>Subreddits</span></h1>
        <p class="uk-text-lead">By default, all are selected</p>

        <div class="uk-margin">
            <input class="uk-input" @input="(e) => changeFilterDebounce(e)" placeholder="Find subreddit">
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
        
        <div class="uk-margin">
            <vk-button class="uk-margin-right" @click="() => selectAll()">Select All</vk-button>
        
            <vk-button @click="() => clearAll()">Clear All</vk-button>
        </div>
    </div>
</template>

<script>
import _ from 'lodash';

export default {
    props: ['allSubreddits'],
    data() {
        return {
            subredditFilter: "",
            subreddits: []
        }
    },
    watch: {
        allSubreddits(newValue, old) {
            if(!newValue || newValue == old)
                return;

            var oldValuesMap = {};

            for(const oldValue of this.subreddits) {
                oldValuesMap[oldValue.name] = oldValue.value;
            }

            this.subreddits = newValue
                .sort((a, b) => a.toLowerCase().localeCompare(b.toLowerCase()))
                .map(s => ({name: s, value: oldValuesMap[s]}));
        }
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
  height: 400px;
  overflow-y: auto;
}

.scroller > label {
  height: 32%;
  padding: 0 12px;
  display: flex;
  align-items: center;
}
</style>
