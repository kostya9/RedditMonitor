<template>
    <div id="selected">
        <div class="uk-text-center">
            <h1 class="uk-heading-primary">
                {{title}}
            </h1>
        </div>
        <vk-grid matched>
            <transition-group name="image-list" tag="div">
                <template v-for="item in images">
                    <top-image v-if="!selectedImage || selectedImage.url !== item.url" :key="item.url + item.count" :image="item" @opened="(i) => selectImage(i)"> </top-image>
                    <selected-image v-else :image="item" :key="item.url + item.count" @closed="() => selectImage(null)">dfdsf</selected-image>
                </template>
            </transition-group>
        </vk-grid>    
    </div>
</template>

<script>
import TopImage from './../components/TopImage.vue'
import SelectedImage from './../components/SelectedImage.vue'
export default {
    name: 'image-list',
    props: ['images', 'title'],
    components: {TopImage, SelectedImage},
    data: function() {
        return {
            selectedImage: null
        }
    },
    methods: {
        selectImage(i) {
            this.selectedImage = i;
        }
    }
}
</script>

<style>
.image-list-enter-active, .image-list-leave-active {
  transition: all 3s;
}
.image-list-enter, .image-list-leave-to /* .list-leave-active below version 2.1.8 */ {
  opacity: 0;
  transform: translateY(30px);
}

.image-list-move {
  transition: transform 1s;
}

.image-list-leave-active {
  position: absolute;
}
</style>