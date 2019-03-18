<template>
  <div id="app">
    <div class="container">
        <h1 class="uk-header-primary">
            Find Similar Images
        </h1>
        <div class="uk-margin">
            <div uk-form-custom>
                <input type="file" accept="image/*" @change="filesChange($event.target.name, $event.target.files);">
                <button class="uk-button uk-button-default" type="button" tabindex="-1">Select image</button>
            </div>
        </div>

        <div uk-grid class="uk-grid-divider">
            <div class="uk-card uk-card-default uk-card-hover uk-width-1-4 uk-width-small-1-1" v-for="(image, index) in images" :key="index">
                <div class="uk-card-header">
                    <img :src="image.imageUrl">
                </div>
                <div class="uk-card-footer">
                    <a class="uk-button uk-button-secondary uk-margin-right" target='_blank' :href="image.imageUrl">
                        Image
                    </a>
                    <a class="uk-button uk-button-secondary" target='_blank' :href="`https://reddit.com${image.url}`">
                        Post
                    </a>
                </div>
            </div>
        </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';
import BaseUrl from './../BaseUrl.js'
export default {
  name: 'HelloWorld',
  props: {
  },
  data() {
    return {
      images: []
    }
  },
  methods: {
    filesChange(n, f) {
      const formData = new FormData();
      formData.append('file',f[0]);
      const basePath = BaseUrl.Value;
      this.images = [];
      axios.post(`${basePath}/api/Similarity`, formData)
        .then(d => {
          this.images = d.data;
        });
    }
  },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}

.similar-img {
  height: 200px;
  margin: 50px;
}
</style>


<style>
#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>