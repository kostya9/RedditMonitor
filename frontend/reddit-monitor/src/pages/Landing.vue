<template>
<div>
    <header class="dbr-hero-header uk-block uk-block-large uk-cover-background uk-flex uk-flex-middle uk-height-viewport uk-contrast">
    <div class="uk-container uk-container-center">
        <section class="uk-grid uk-grid-match" data-uk-grid-margin="">
            <div class="uk-width-medium-1-1">
                <div class="uk-panel  uk-text-center" data-uk-parallax="{ opacity: '1,0', y:'-150' }">
                <h3>Reddit Monitor</h3>
                <p>Monitoring how images are used in Reddit social network</p>
                <a class="uk-button-large uk-margin-top" href="#" @click="show = true">Start</a>
                </div>
            </div>
        </section>
    </div>
    </header>
    <vk-modal :show.sync="show">
         <SignForm></SignForm>
    </vk-modal>
</div>
</template>

<script>
import SignForm from './../components/SignForm'

export default {
    components: {
        SignForm
    },
    data: function() {
        return {
            show: false
        }
    }
}
</script>

<style lang="scss" scoped>
@import url(https://fonts.googleapis.com/css?family=Raleway:100,200,300,400,500,600,700,800,900);
*{
    font-family: 'Raleway', sans-serif;
    font-weight:300;
    font-size: 16px;
}
h3{
	font-size: 50px;
	line-height: 60px;
	font-weight: 400;
	color: white;
   font-family: 'Raleway', sans-serif;
   font-weight:300;
   letter-spacing: 5px;
}

/* HERO HEADER */

header {
    color: white;
	&.dbr-hero-header {
        background: 
            linear-gradient(
                rgba(0, 0, 0, 0.6),
                rgba(0, 0, 0, 0.6)
            ),
            url("./../assets/landing.jpg");
		background-attachment: scroll;
	}
	a {
     $main-color: #f7b243;
		&:link, &:visited {
            color: white;
			letter-spacing: 2px;
			display: inline-block;
			height: 55px;
			padding:0px 40px;
			background: $main-color;
			border: 0;
			border-radius: 5px;
			font-size: 15px;
			line-height: 55px;
			text-transform: uppercase;
			cursor: pointer;
			-moz-transition: background 0.6s;
			-o-transition: background 0.6s;
			-webkit-transition: background 0.6s;
			transition: background 0.6s;
		}
		&:hover {
			$hero-rgba-color: rgba(red($main-color), green($main-color), blue($main-color), 0.5);
			background-color: $hero-rgba-color;
			text-decoration: none;
		}
	}
}

/* HERO HEADER */
</style>
