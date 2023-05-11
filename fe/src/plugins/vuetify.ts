/**
 * plugins/vuetify.ts
 *
 * Framework documentation: https://vuetifyjs.com`
 */

// Styles
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'

// Composables
import {createVuetify} from 'vuetify'

// https://vuetifyjs.com/en/introduction/why-vuetify/#feature-guides
export default createVuetify({
  theme: {
    defaultTheme: 'dark',
    themes: {
      light: {
        colors: {
          surface: '#cccccc',
          primary: '#dadada',
          background: '#dadada',
          secondary_background: '#e8e8e8',
          bar: '#e8e8e8',
          text: '#000000',
          button: '#3F9E4A',
          error: '#cc0000'
        },
      },
      dark: {
        colors: {
          surface: '#1e1e1e',
          primary: '#1e1e1e',
          background: '#000000',
          secondary_background: '#424242',
          bar: '#212121',
          text: '#FFFFFF',
          button: '#3F9E4A',
          error: '#cc0000'
        }
      }
    },
  },
})
