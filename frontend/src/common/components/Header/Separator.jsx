import clsx from 'clsx';
import styles from './Header.module.scss';

const Separator = ({ className = "" }) => {
    return <div className={clsx(styles.separator, className)} />;
  };
  
  export default Separator;
  