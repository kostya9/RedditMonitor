import Vue from 'vue';

export default class AsyncStore extends Vue {

    constructor() {
        super();
        this.calls = 0;
        this.requests = [];
    }

    sendCall() {
        if(this.calls === 0) {
            this.$emit('asyncStateChanged', true);
        }

        this.calls++;
    }

    receiveCall() {
        this.calls--;

        if(this.calls === 0) {
            this.$emit('asyncStateChanged', false);
        }
    }
    
    onAsyncStateChanged(func) {
        this.$on('asyncStateChanged', func);
    }
}