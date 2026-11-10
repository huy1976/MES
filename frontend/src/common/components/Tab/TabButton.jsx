import clsx from 'clsx';
import styles from './Tab.module.scss';


const TabButton = ({ children, active, className = "", ...props }) => {
    return (
      <div
        className={clsx(styles.tabButton, active ? styles.tabButton_active : styles.tabButton_inactive, className)}
        {...props}
      >
        {children}
      </div>
    );
  };
  
  export default TabButton;
  