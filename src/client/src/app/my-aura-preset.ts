import { definePreset } from '@primeng/themes';
import Aura from '@primeng/themes/aura'


const MyAuraPreset = definePreset(Aura, {
  semantic: {
    colorScheme: {
      light: {
        surface: {
          0: '#f5f0e9',  
          100: '#e8dfd1'
        }
      },
      dark: {
        surface: {
          0: '#2c2a29',  
          100: '#3a3735'
        }
      }
    }
  },
  components: {
    button: {
      colorScheme: {
        light: { root: { background: 'transparent', borderColor: '#b1a999', color: '#6f5e41' } },
        dark: { root: { background: 'transparent', borderColor: '#776e5e', color: '#ddcda5' } }
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
    textarea: {
      colorScheme: {
        light: { root: { background: '#f5f0e9', borderColor: '#cdc1b4' } },
        dark: { root: { background: '#3a3735', borderColor: '#584f44' } }
      }
    },
    dropdown: {
      colorScheme: {
        light: { root: { background: '#f5f0e9', borderColor: '#cdc1b4' } },
        dark: { root: { background: '#3a3735', borderColor: '#584f44' } }
      }
    }
  }
});

export default MyAuraPreset;
