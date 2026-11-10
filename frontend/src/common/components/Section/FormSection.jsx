import clsx from 'clsx';
import styles from './Section.module.scss';

const FormSection = ({ className = "", children, style }) => {
    return (
      <div className={clsx(styles.formSection, className)} style={style}>
        {children}
      </div>
    );
  };
  
  export default FormSection;
  