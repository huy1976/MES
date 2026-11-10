import clsx from 'clsx';
import styles from './Text.module.scss';

const TextNote = ({ children, className = "", style}) => {
    return (
      <h3 className={clsx(styles.textNote, className)} style={style}>
        {children}
      </h3>
    );
  };
  
  export default TextNote;
  