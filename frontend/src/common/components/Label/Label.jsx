import clsx from 'clsx';
import styles from './Label.module.scss';

const Label = ({ className = "", children, style }) => {
    return <div className={clsx(styles.label, className)} style={style}>{children}</div>;
  };
  
  export default Label;
  