import styles from './ActionButton.module.scss';
import clsx from 'clsx';

const ActionButton = ({ children, onClick, className, style }) => {
    return (
      <button
        onClick={onClick}
        className={clsx(styles.actionButton, className)}
        style = {style}
      >
        {children}
      </button>
    );
  };
  
  export default ActionButton;
  