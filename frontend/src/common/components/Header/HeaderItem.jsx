import clsx from 'clsx';
import styles from './Header.module.scss';

const HeaderItem = ({ children, className = "" }) => {
    return <div className={clsx(styles.headerItem, className)}>{children}</div>;
  };
  
  export default HeaderItem;
  