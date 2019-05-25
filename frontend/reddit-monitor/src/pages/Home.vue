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

        <image-list v-if="images.length" :images="images" title="Found images"></image-list>
    </div>
  </div>
</template>

<script>
import axios from 'axios';
import BaseUrl from './../BaseUrl.js'
import ImageList from './../components/ImageList.vue'

export default {
  name: 'HelloWorld',
  props: {
  },
  components: {
    ImageList
  },
  data() {
    return {
      images: [],
      selectedSubreddits: []
    }
  },
  methods: {
    filesChange(n, f) {
      if(!f || !f.length || !f[0])
        return;

      const formData = new FormData();
      formData.append('file',f[0]);
      for (var i = 0; i < this.selectedSubreddits.length; i++) {
        formData.append('subreddits[]', this.selectedSubreddits[i]);
      }
      const basePath = BaseUrl.Value;
      this.images = [];
      axios.post(`${basePath}/api/Similarity`, formData)
        .then(d => {
          this.images = d.data;
        });
    }
  },
  created() {
    this.selectedSubreddits = this.$subredditStore.getSubreddits();
    this.$subredditStore.subscribeOnSubredditsChanged((s) => {
        this.selectedSubreddits = s;
    });
  }
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