<template>
  <div>
    <WaitingRoom />
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import WaitingRoom from '@/components/WaitingRoom.vue';

@Component({
  components: {
    WaitingRoom,
  },
})
export default class Play extends Vue {

}
</script>
