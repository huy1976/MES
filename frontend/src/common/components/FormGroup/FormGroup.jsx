import clsx from 'clsx';
import styles from './FormGroup.module.scss';

const FormGroup = ({ children, className = "", style}) => {
    return <div className={clsx(styles.formGroup, className)} style={style}
    >
      {children}
      </div>;
  };
  
  export default FormGroup;
  