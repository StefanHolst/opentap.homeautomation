<template>
  <div class="grid">
    <div class="light" v-for="plan in testPlans" :key="plan" @click="runPlan(plan)">
      <h3>{{plan}}</h3>
    </div>
  </div>
</template>

<script lang="ts">
import { Options, Vue } from 'vue-class-component';
import axios from 'axios';

@Options({
  props: {
    msg: String
  }
})
export default class HelloWorld extends Vue {
  public testPlans: string[] = [];

  async mounted(){
    // Get available test plans
    this.testPlans = await (await axios.get("/getplans")).data.split("\n").filter((p:string) => !!p).map((p:string) => p.substr(0, p.length - 8));
  }

  public async runPlan(plan: string){
    console.log(plan);
    await axios.get("/runplan?plan=" + plan)
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
.grid{
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
  justify-content: center;
}

.light{
  width: 100px;
  height: 100px;
  border-radius: 5px;
  background-color: rgb(229, 30, 51);
  color: white;
}
</style>
