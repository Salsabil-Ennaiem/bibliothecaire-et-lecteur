import { definePreset } from '@primeng/themes';
import Aura from '@primeng/themes/aura'


const MyAuraPreset = definePreset(Aura, {
  semantic: {
    colorScheme: {
      light: { surface: { 0: '#ffe4e1', 100: '#f8d7da' } },
      dark: { surface: { 0: '#1a1a1a', 100: '#333333' } }
    }
  },
  components: {
    button: {
      colorScheme: {
        light: {
          primary: {
            root: { background: 'orange', borderColor: 'orange', color: '#fff' },
            hover: { background: '#ffb347', borderColor: '#ffb347', color: '#fff' },
            active: { background: '#e69500', borderColor: '#e69500', color: '#fff' }
          }
        },
        dark: {
          primary: {
            root: { background: 'purple', borderColor: 'purple', color: '#fff' },
            hover: { background: '#9b30ff', borderColor: '#9b30ff', color: '#fff' },
            active: { background: '#7f00ff', borderColor: '#7f00ff', color: '#fff' }
          }
        }
      }
    },
    inputtext: {
      colorScheme: {
        light: { root: { background: '#f5f0e9', borderColor: '#cdc1b4' } },
        dark: { root: { background: '#fdf4eeff', borderColor: '#b6aa9cff' } }
      }
    },

    card: {
      colorScheme: {
        light: { root: { background: '#f5f0e9', color: '#4a3f35' } },
        dark: { root: { background: '#353332ff', color: '#dfd9cc' } }
      }
    },
    select: {
      colorScheme: {
        light: { root: { background: '#f5f0e9', color: '#9c3b71ff' }, option: { background: '#f5f0e9', color: '#21b128ff' } },
        dark: { root: { background: '#58525844', borderColor: '#cdc1b4' }, option: { background: '#f5f0e9', color: '#dac347ff' } }
      }
    },
    textarea: {
      colorScheme: {
        light: { root: { background: '#f5f0e9', borderColor: '#584f44' } },
        dark: { root: { background: '#754f36ff', borderColor: '#cdc1b4' } }
      }
    }

  }
});

export default MyAuraPreset;
