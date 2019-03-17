<template>
  <div id="app">
    <div class="container">
      <!--UPLOAD-->
      <form enctype="multipart/form-data">
        <h1>Find similiar</h1>
        <div class="dropbox">
          <input type="file" @change="filesChange($event.target.name, $event.target.files);"
            accept="image/*" class="input-file">
        </div>
      </form>

      <div>
        <div v-for="a in images" :key="a.id">
          <a :href="'https://reddit.com/' +a.url">Link</a>
          <img :src="a.imageUrl"  class="similar-img"/>
        </div>
        </div>
      </div>
  </div>
</template>

<script>
import axios from 'axios';
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
      //const basePath = '';
      const basePath = 'http://localhost:64621';
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