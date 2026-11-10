import clsx from 'clsx';
import styles from './DeleteButton.module.scss';

const DeleteButton = ({ children, onClick, className}) => {
    return (
      <button
        onClick={onClick}
        className={clsx(styles.deleteButton, className)}
      >
        {children}
      </button>
    );
  };
  export default DeleteButton;