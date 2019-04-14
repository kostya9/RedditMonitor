import Vue from 'vue'

export default class NotificationStore extends Vue {
    error(text) {
        this.$emit('notification', {status: 'danger', message: text});
    }

    success(text) {
        this.$emit('notification', {status: 'success', message: text});
    }

    onNewNotification(f) {
        this.$on('notification', f);
    }
}