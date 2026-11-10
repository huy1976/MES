import clsx from 'clsx';
import styles from './Tab.module.scss';

const TabContainer = ({ children, className = "" }) => {
    return <div className={clsx(styles.tabContainer, className)}>{children}</div>;
  };
  
  export default TabContainer;
  