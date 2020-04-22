<template>
  <div>
    <div>Vote Track</div>
    <div class="uk-grid-divider uk-grid-medium uk-child-width-auto uk-flex-center" uk-grid>
      <div v-for="i in partyNumberList" :key="i"
        :class="getClassesForPartyNumber(i+1)">
        {{i+1}}
      </div>
    </div>
    <div v-if="partyNumber === 5" class="uk-margin-small-top">
      Evil will win if this party fails!
    </div>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { State } from 'vuex-class';

@Component
export default class PartNumber extends Vue {
  @State private partyNumber!: number;

  private partyNumberList = [...new Array(5).keys()];

  getClassesForPartyNumber(partyNumber: number) {
    const classes = [];
    if (partyNumber === this.partyNumber) {
      classes.push('uk-text-bold');
    } else {
      classes.push('uk-text-muted');
    }
    if (partyNumber === 5) {
      classes.push('uk-text-danger');
    }
    return classes;
  }
}
</script>
