import clsx from 'clsx';
import styles from './DropdownIcon.module.scss';

const DropdownIcon = ({ className = "", children, style }) => {
    return (
      <div
        className={clsx(styles.dropDownIcon, className)} style={style}
      >
        {children}
      </div>
    );
  };
  
  export default DropdownIcon;
  