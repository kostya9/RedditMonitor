<template>
    <div class="uk-width-1-1 uk-margin uk-section uk-section-secondary uk-padding">
        <div class="uk-container">
        <vk-grid matched>
            <div class="uk-width-1-3">
                <span class="selected-image-container">
                    <img :src="image.url" class="selected-image">
                </span>
                <div class="uk-panel uk-panel-scrollable commentsscroll"> 
                    <div v-for="comment in image.comments" :key="comment">
                        <vk-button-link :class="{'uk-button-primary': selectedComment == comment}" @click="() => selectComment(comment)">
                            {{getPublicationName(comment)}}
                        </vk-button-link>
                    </div>
                </div>
            </div>
            <div class="uk-width-2-3">
                <div class="container redditpost">
                    <h2>{{selectedCommentTitle}}</h2>

                    <p>{{selectedCommentText}}</p>

                    <vk-button-link target='_blank' :href="selectedComment">{{selectedComment}}</vk-button-link>
                </div>
            </div>
        </vk-grid>
        </div>
    </div>
</template>

<script>
import axios from 'axios';

export default {
    name: 'top-image',
    props: ['image'],
    data: function() {
        return {
            selectedComment: null,
            selectedCommentTitle: null,
            selectedCommentText: null
        }
    },
    watch: {
        image: {
            immediate: true,
            handler: function (cur) {
                if(!cur)
                    return;

                setTimeout(() => this.selectComment(cur.comments[0]), 1000);
            }
        }
    },
    methods: {
        selectComment(c) {
            if(this.selectedComment == c)
                return;
            
            this.selectedComment = c;
            axios.post('/api/redditdata/findpost', {url: c})
                .then(r => {
                    this.selectedCommentTitle = r.data.title;
                    this.selectedCommentText = r.data.text;
                });
        },
        getPublicationName(c) {
            var split = c.split('/').filter(s => s.length !== 0);
            var underscoreName = split[split.length - 1];
            var underscoreSplit = underscoreName.split('_');

            var name = underscoreSplit.join(' ');
            return name + " in " + split[3];
        }
    }
}
</script>

<style lang="scss">

.selected-image-container {
    text-align: center;
}

.selected-image {
    border: 1px white;
    max-height: 400px;
    object-fit: contain;
}

.uk-button {
    word-wrap: break-word;
}

div.commentsscroll {
    max-height: 300px;
    white-space: normal;

    & .uk-button {
        white-space: normal;
    }
}

.redditpost {
    display: flex;
    flex-direction: column;
    p {
        flex-grow: 1;
    }
}


</style>
