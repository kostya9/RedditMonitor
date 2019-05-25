import Vue from 'vue';

export default class AsyncStore extends Vue {

    constructor() {
        super();
        this.calls = 0;
        this.requests = [];
    }

    sendCall() {
        console.log('plus');
        if(this.calls === 0) {
            this.$emit('asyncStateChanged', true);
        }

        this.calls++;
    }

    receiveCall() {
        console.log('minus');
        this.calls--;

        if(this.calls === 0) {
            this.$emit('asyncStateChanged', false);
        }
    }
    
    onAsyncStateChanged(func) {
        console.log('sub');
        this.$on('asyncStateChanged', func);
    }
}