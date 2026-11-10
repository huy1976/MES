import clsx from 'clsx';
import styles from './CreateButton.module.scss';
const CreateButton = ({ children, className, ...rest}) => {
    return (
      <button
        {...rest}
        className={clsx(styles.createButton, className)}
      >
        {children}
      </button>
    );
  };
  export default CreateButton;
  