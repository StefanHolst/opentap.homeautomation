<template>
  <h1>OpenTAP Home Automation</h1>
  <TestPlans/>
</template>

<script lang="ts">
import { Options, Vue } from 'vue-class-component';
import TestPlans from './components/TestPlans.vue';

@Options({
  components: {
    TestPlans,
  },
})
export default class App extends Vue {}
</script>

<style lang="scss">
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
